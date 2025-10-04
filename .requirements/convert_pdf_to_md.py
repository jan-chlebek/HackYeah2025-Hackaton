from __future__ import annotations

import argparse
import re
from pathlib import Path
from typing import NamedTuple

import fitz  # PyMuPDF


class TextBlock(NamedTuple):
    """Represents a block of text with formatting metadata."""
    text: str
    font_size: float
    font_name: str
    is_bold: bool
    is_italic: bool
    page_num: int
    bbox: tuple[float, float, float, float]


def extract_text_blocks(page: fitz.Page, page_num: int) -> list[TextBlock]:
    """Extract text blocks with formatting information."""
    blocks: list[TextBlock] = []
    text_dict = page.get_text("dict")
    
    for block in text_dict.get("blocks", []):
        if block.get("type") != 0:  # Skip non-text blocks
            continue
            
        for line in block.get("lines", []):
            for span in line.get("spans", []):
                text = span.get("text", "").strip()
                if not text:
                    continue
                    
                font_size = span.get("size", 12.0)
                font_name = span.get("font", "").lower()
                is_bold = "bold" in font_name or span.get("flags", 0) & 2 ** 4
                is_italic = "italic" in font_name or span.get("flags", 0) & 2 ** 1
                bbox = span.get("bbox", (0, 0, 0, 0))
                
                blocks.append(TextBlock(
                    text=text,
                    font_size=font_size,
                    font_name=font_name,
                    is_bold=is_bold,
                    is_italic=is_italic,
                    page_num=page_num,
                    bbox=bbox
                ))
    
    return blocks


def detect_heading_level(block: TextBlock, avg_font_size: float, max_font_size: float) -> int | None:
    """
    Detect if a text block is a heading and determine its level.
    Returns None if not a heading, or 1-6 for heading levels.
    """
    # Check if text looks like a heading (short, possibly numbered, larger font)
    text = block.text.strip()
    
    # Skip if too long to be a heading
    if len(text) > 150:
        return None
    
    # Check for numbered sections (e.g., "1.", "1.1", "A.", "I.")
    numbered_pattern = r'^(\d+\.|\d+\.\d+\.?|\d+\.\d+\.\d+\.?|[A-Z]\.|\b[IVX]+\.|[a-z]\)|\(\d+\))'
    is_numbered = bool(re.match(numbered_pattern, text))
    
    # Check for ALL CAPS headings
    is_all_caps = text.isupper() and len(text.split()) >= 2
    
    # Calculate relative font size
    size_ratio = block.font_size / avg_font_size if avg_font_size > 0 else 1.0
    
    # Determine heading level based on multiple factors
    if block.font_size >= max_font_size * 0.9 or size_ratio >= 1.5:
        return 1  # Main heading
    elif (size_ratio >= 1.3 or block.is_bold) and (is_numbered or is_all_caps or len(text.split()) <= 8):
        return 2  # Major section
    elif size_ratio >= 1.15 or (block.is_bold and (is_numbered or len(text.split()) <= 6)):
        return 3  # Subsection
    elif block.is_bold and is_numbered:
        return 4  # Minor subsection
    
    return None


def clean_text(text: str) -> str:
    """Clean and normalize text."""
    # Replace bullet characters with markdown list markers
    for needle in ("•", "−", "–", "➢", "◦", "●", "■", "□"):
        text = text.replace(needle, "-")
    
    # Normalize whitespace
    text = re.sub(r'\s+', ' ', text)
    
    return text.strip()


def merge_lines_into_paragraphs(blocks: list[TextBlock]) -> list[str]:
    """Merge text blocks into coherent paragraphs."""
    if not blocks:
        return []
    
    paragraphs: list[str] = []
    current_para: list[str] = []
    last_block = None
    
    for block in blocks:
        # Start new paragraph if:
        # - Font changes significantly
        # - Large vertical gap
        # - Line starts with list marker or number
        starts_new_para = (
            last_block is None or
            abs(block.font_size - last_block.font_size) > 1.0 or
            block.bbox[1] - last_block.bbox[3] > last_block.font_size * 1.5 or
            re.match(r'^[-*+]|\d+\.|\([a-z0-9]+\)', block.text)
        )
        
        if starts_new_para and current_para:
            paragraphs.append(' '.join(current_para))
            current_para = []
        
        current_para.append(block.text)
        last_block = block
    
    if current_para:
        paragraphs.append(' '.join(current_para))
    
    return paragraphs


def convert(pdf_path: Path) -> None:
    if not pdf_path.exists():
        raise FileNotFoundError(f"PDF not found: {pdf_path}")

    md_path = pdf_path.with_suffix(".md")
    assets_dir = md_path.parent / f"{pdf_path.stem}_assets"
    assets_dir.mkdir(exist_ok=True)

    doc = fitz.open(pdf_path)
    
    # First pass: collect all text blocks and extract images
    all_blocks: list[TextBlock] = []
    image_references: dict[int, list[str]] = {}  # page_num -> list of image markdown
    
    for page_index, page in enumerate(doc, start=1):
        blocks = extract_text_blocks(page, page_index)
        all_blocks.extend(blocks)
        
        # Extract embedded raster images from this page
        images = page.get_images(full=True)
        if images:
            if page_index not in image_references:
                image_references[page_index] = []
            for image_index, image in enumerate(images, start=1):
                xref = image[0]
                try:
                    base_image = doc.extract_image(xref)
                    image_bytes = base_image["image"]
                    image_ext = base_image["ext"]
                    image_path = assets_dir / f"{pdf_path.stem}_page{page_index}_img{image_index}.{image_ext}"
                    with open(image_path, "wb") as img_file:
                        img_file.write(image_bytes)
                    rel_path = image_path.relative_to(md_path.parent).as_posix()
                    image_references[page_index].append(f"![Figure {page_index}.{image_index}]({rel_path})")
                except Exception as e:
                    print(f"⚠ Warning: Could not extract image {image_index} from page {page_index}: {e}")
        
        # Render every page as an image to capture all visual content
        try:
            # Render the page at high resolution
            mat = fitz.Matrix(2.0, 2.0)  # 2x zoom for better quality
            pix = page.get_pixmap(matrix=mat, alpha=False)
            
            # Save as PNG
            image_path = assets_dir / f"{pdf_path.stem}_page{page_index}_render.png"
            pix.save(str(image_path))
            rel_path = image_path.relative_to(md_path.parent).as_posix()
            
            if page_index not in image_references:
                image_references[page_index] = []
            image_references[page_index].append(f"![Page {page_index} rendering]({rel_path})")
            
            # Check if page has vector graphics for reporting
            drawings = page.get_drawings()
            if drawings and len(drawings) > 5:
                print(f"  Rendered page {page_index} with {len(drawings)} vector drawings")
        except Exception as e:
            print(f"⚠ Warning: Could not render page {page_index}: {e}")
    
    # Calculate font statistics
    if all_blocks:
        font_sizes = [b.font_size for b in all_blocks]
        avg_font_size = sum(font_sizes) / len(font_sizes)
        max_font_size = max(font_sizes)
    else:
        avg_font_size = 12.0
        max_font_size = 12.0
    
    # Second pass: process pages with section detection
    markdown_lines: list[str] = []
    current_page = 0
    
    for block in all_blocks:
        # Add page marker when page changes
        if block.page_num != current_page:
            current_page = block.page_num
            if markdown_lines:  # Don't add before first page
                markdown_lines.append("")
            markdown_lines.append(f"<!-- Page {current_page} -->")
            markdown_lines.append("")
            
            # Add images for this page if any exist
            if current_page in image_references:
                for img_ref in image_references[current_page]:
                    markdown_lines.append(img_ref)
                    markdown_lines.append("")
        
        # Detect if this is a heading
        heading_level = detect_heading_level(block, avg_font_size, max_font_size)
        
        clean_block_text = clean_text(block.text)
        
        if heading_level:
            # Add heading with appropriate level
            markdown_lines.append("")
            markdown_lines.append(f"{'#' * heading_level} {clean_block_text}")
            markdown_lines.append("")
        else:
            # Regular text
            markdown_lines.append(clean_block_text)
    
    # Store page count and image count before closing
    page_count = len(doc)
    total_images = sum(len(imgs) for imgs in image_references.values())
    doc.close()

    # Build final markdown document
    header = [
        f"# {pdf_path.stem.replace('_', ' ')}",
        "",
        "> Converted from PDF automatically with section detection.",
        "> Verify formatting, links, and section hierarchy manually.",
        ""
    ]

    final_content = "\n".join(header + markdown_lines)
    
    # Clean up excessive blank lines
    final_content = re.sub(r'\n{4,}', '\n\n\n', final_content)
    
    md_path.write_text(final_content, encoding="utf-8")
    print(f"✓ Wrote markdown to {md_path}")
    print(f"✓ Assets saved to {assets_dir}")
    print(f"✓ Processed {page_count} pages with {len(all_blocks)} text blocks")
    if total_images > 0:
        print(f"✓ Extracted {total_images} images")


def main() -> None:
    parser = argparse.ArgumentParser(description="Convert a PDF into Markdown and extract images.")
    parser.add_argument("pdf", type=Path, help="Path to the PDF file")
    args = parser.parse_args()
    convert(args.pdf)


if __name__ == "__main__":
    main()
