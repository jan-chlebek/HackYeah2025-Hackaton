# PDF to Markdown Converter - Documentation

## Purpose & Rationale

This tool was created specifically for the **HackYeah 2025 #Prompt2Code2** competition to convert competition requirement PDFs into a format that's optimized for AI-assisted development and prompt engineering.

### Why This Tool Was Created

1. **AI/LLM Context Integration**
   - PDF files cannot be easily processed by AI coding assistants like GitHub Copilot
   - Markdown format allows AI tools to understand and reference competition requirements
   - Structured sections enable targeted prompting and context-aware code generation

2. **Searchability & Navigation**
   - Raw PDFs lack searchable section hierarchy
   - Intelligent heading detection creates proper document structure (H1-H6)
   - Enables quick navigation to specific requirements and features

3. **Complete Information Preservation**
   - Competition PDFs contain critical visual content (tables, UI mockups, diagrams)
   - Text-only extraction would lose important design specifications
   - Full page rendering ensures nothing is missed

4. **Prompt Engineering Documentation**
   - Competition requires documenting the AI-assisted development process
   - Having requirements in markdown makes them easier to reference in prompts
   - Visual content can be referenced when generating UI/UX code

## How It Works

### Architecture Overview

The converter operates in two main passes:

```
Pass 1: Data Collection & Image Extraction
  ├─ Extract text blocks with formatting metadata (font size, style, position)
  ├─ Extract embedded raster images (if any)
  └─ Render every page as high-resolution PNG (2x scale)

Pass 2: Structure Detection & Markdown Generation
  ├─ Analyze font statistics (average size, maximum size)
  ├─ Detect headings based on multiple heuristics
  ├─ Apply intelligent section hierarchy (H1-H4)
  └─ Generate clean markdown with embedded images
```

### Key Components

#### 1. TextBlock Extraction (`extract_text_blocks`)

**What it does:**
- Extracts every text span from the PDF with rich metadata
- Captures font size, font name, bold/italic status, and bounding box position
- Preserves page number for proper organization

**Why it's important:**
- Formatting information is crucial for detecting headings
- Position data helps identify visual structure
- Enables intelligent document reconstruction

**Technical details:**
```python
class TextBlock(NamedTuple):
    text: str                  # The actual text content
    font_size: float          # Used for heading detection
    font_name: str            # Detects bold/italic fonts
    is_bold: bool             # Heading indicator
    is_italic: bool           # Styling information
    page_num: int             # Page organization
    bbox: tuple               # Spatial position (future use)
```

#### 2. Heading Detection (`detect_heading_level`)

**What it does:**
- Analyzes text blocks to determine if they are headings
- Assigns appropriate markdown heading levels (H1-H4)
- Uses multiple heuristics for accurate detection

**Detection Criteria:**

| Level | Criteria |
|-------|----------|
| H1 | Font size ≥ 90% of max, or 1.5× average size |
| H2 | Font size ≥ 1.3× average, or bold + (numbered/ALL CAPS/short) |
| H3 | Font size ≥ 1.15× average, or bold + numbered |
| H4 | Bold + numbered pattern |

**Pattern Recognition:**
- Numbered sections: `1.`, `1.1`, `1.1.1`, `A.`, `I.`, `(1)`, `a)`
- ALL CAPS headings: `FUNKCJONALNOŚCI PREFEROWANE`
- Short bold text: 6-8 words or less

**Why this approach:**
- PDFs don't contain semantic heading information
- Visual formatting indicates document structure
- Multiple criteria ensure accuracy across different PDF styles

#### 3. Image Handling

**Two-tier approach:**

1. **Embedded Raster Images** (`get_images()`)
   - Extracts JPG, PNG images embedded in PDF
   - Preserves original quality and format
   - Most PDFs don't use this method

2. **Full Page Rendering** (`get_pixmap()`)
   - Renders entire page as PNG at 2× resolution (high quality)
   - Captures vector graphics, tables, diagrams, UI mockups
   - Ensures complete visual information preservation

**Why render every page:**
- Competition PDFs contain critical visual specifications
- Vector graphics (tables, mockups) aren't extractable as images
- UI/UX requirements need visual reference
- Better to have complete information than miss critical details

**Resolution choice:**
- 2× zoom (Matrix(2.0, 2.0)) balances quality vs file size
- Readable text in diagrams and tables
- Sufficient detail for UI implementation

#### 4. Text Cleaning (`clean_text`)

**Normalizations applied:**
- Bullet characters → markdown list markers (`-`)
- Multiple whitespace → single space
- Trim leading/trailing whitespace

**Preserved characters:**
```python
Converted: • − – ➢ ◦ ● ■ □ → -
Normalized: multiple spaces → single space
```

**Why this matters:**
- Consistent list formatting in markdown
- Cleaner text for AI processing
- Better readability

### Output Structure

#### Markdown File Format

```markdown
# [Document Title]

> Converted from PDF automatically with section detection.
> Verify formatting, links, and section hierarchy manually.

<!-- Page 1 -->

![Page 1 rendering](assets/document_page1_render.png)

## Section Heading

Regular text content...

### Subsection

More content...

<!-- Page 2 -->

![Page 2 rendering](assets/document_page2_render.png)

...
```

#### Assets Directory

```
document_assets/
├── document_page1_render.png    # Full page rendering
├── document_page2_render.png
├── document_page3_img1.jpg      # Embedded images (if any)
└── ...
```

## Usage

### Basic Usage

```bash
python convert_pdf_to_md.py <path-to-pdf>
```

### Example

```bash
python convert_pdf_to_md.py DETAILS_UKNF_Prompt2Code2.pdf
```

### Output

```
  Rendered page 3 with 158 vector drawings
  Rendered page 4 with 93 vector drawings
  ...
✓ Wrote markdown to DETAILS_UKNF_Prompt2Code2.md
✓ Assets saved to DETAILS_UKNF_Prompt2Code2_assets
✓ Processed 40 pages with 2689 text blocks
✓ Extracted 40 images
```

## Technical Requirements

### Dependencies

```python
PyMuPDF (fitz)  # PDF processing and rendering
```

Install with:
```bash
pip install PyMuPDF
```

### Python Version

- Python 3.10+ (uses `type | None` syntax)
- Type hints with `NamedTuple` and `typing` module

### Performance Considerations

**Memory Usage:**
- Loads entire PDF into memory
- Stores all text blocks before processing
- Peak usage: ~50-100MB per 40-page PDF

**Processing Time:**
- ~1-2 seconds per page for rendering
- Text extraction is fast (<1 second for 40 pages)
- Total: ~1-2 minutes for typical competition PDF

**File Size:**
- PNG renders: ~200-500KB per page at 2× resolution
- Total assets: ~10-20MB for 40-page document
- Markdown file: ~100-200KB text

## Design Decisions & Trade-offs

### Decision: Render Every Page as Image

**Rationale:**
- Competition PDFs contain critical UI mockups and diagrams
- Cannot risk missing visual information
- Text is searchable via markdown; images provide visual context

**Trade-off:**
- Larger file sizes vs complete information
- **Choice:** Completeness over size (storage is cheap, missing info is costly)

### Decision: 2× Resolution Rendering

**Rationale:**
- Text in diagrams must be readable
- Tables need clear visibility
- Balance between quality and file size

**Trade-off:**
- Higher resolution = larger files but better readability
- **Choice:** 2× provides excellent quality at reasonable size

### Decision: Multi-Heuristic Heading Detection

**Rationale:**
- PDFs lack semantic structure
- Different documents use different heading styles
- Need robust detection across variations

**Trade-off:**
- Complex logic vs simple font-size-only approach
- **Choice:** Better accuracy is worth the complexity

### Decision: Preserve Page Markers

**Rationale:**
- Easy reference back to original PDF
- Debugging and verification
- Helps locate specific requirements

**Trade-off:**
- Slightly clutters markdown vs clean navigation
- **Choice:** Practical utility outweighs aesthetic concerns

## Validation & Quality Assurance

### Verification Steps

After conversion, verify:

1. **Text Accuracy**
   - Compare key sections with original PDF
   - Check special characters and formatting
   - Verify lists and bullet points

2. **Section Hierarchy**
   - Main headings should be H1/H2
   - Subsections properly nested
   - Numbered sections correctly identified

3. **Image Completeness**
   - All pages rendered
   - Images properly embedded
   - Visual content readable

4. **Markdown Validity**
   - Preview in markdown viewer
   - Check image paths are relative
   - Verify links work

### Known Limitations

1. **Table Structure**
   - Tables rendered as images, not markdown tables
   - Text extraction doesn't preserve table layout
   - **Mitigation:** Images capture table visually

2. **Multi-Column Layouts**
   - Text extracted left-to-right, may mix columns
   - **Mitigation:** Page images show correct layout

3. **Footnotes & References**
   - May not maintain proper association
   - **Mitigation:** Page context preserved

4. **Complex Formatting**
   - Colored text, highlights, annotations lost in text
   - **Mitigation:** Page images preserve all formatting

## Competition-Specific Benefits

### For #Prompt2Code2 Challenge

1. **AI Context Integration**
   ```
   Prompt: "Based on the requirements in DETAILS_UKNF_Prompt2Code2.md, 
           create the authentication module..."
   
   AI can now read and understand the requirements directly.
   ```

2. **Prompt Engineering Documentation**
   - Easy to reference specific requirements in prompts
   - Can copy/paste requirement sections into AI conversations
   - Visual mockups available for UI generation prompts

3. **Team Collaboration**
   - Developers can quickly search for features
   - Navigate to relevant sections instantly
   - Share specific requirement links

4. **Compliance Verification**
   - Checklist creation from markdown structure
   - Track implementation against requirements
   - Easy cross-referencing

## Future Enhancements (Potential)

### Possible Improvements

1. **Table Recognition**
   - OCR table structure to markdown tables
   - Preserve data relationships

2. **Smart Image Placement**
   - Only render pages with significant graphics
   - Skip text-only pages to reduce size

3. **Enhanced Heading Detection**
   - Machine learning model for heading detection
   - Learn from user corrections

4. **Interactive Mode**
   - Preview sections during conversion
   - Manual heading level adjustment
   - Section merging/splitting

5. **Metadata Extraction**
   - TOC generation
   - Automatic index creation
   - Cross-reference detection

## Maintenance & Updates

### File Location
```
.requirements/
├── convert_pdf_to_md.py         # Main converter script
├── convert_pdf_to_md.document   # This documentation
├── DETAILS_UKNF_Prompt2Code2.pdf
├── DETAILS_UKNF_Prompt2Code2.md
├── DETAILS_UKNF_Prompt2Code2_assets/
├── RULES_UKNF_Prompt2Code2.pdf
├── RULES_UKNF_Prompt2Code2.md
└── RULES_UKNF_Prompt2Code2_assets/
```

### Version History

**v1.0** - Initial text-only conversion
- Basic text extraction
- Simple bullet point replacement

**v2.0** - Section detection
- Intelligent heading detection
- Font-based structure analysis
- Multi-heuristic approach

**v3.0** - Image support (current)
- Embedded image extraction
- Vector graphics detection (>5 drawings)
- Conditional page rendering

**v4.0** - Full rendering (current)
- Every page rendered as image
- Complete visual preservation
- Enhanced reporting

## Conclusion

This converter transforms competition requirement PDFs into an AI-friendly format that enables:

✅ Efficient prompt engineering with contextual requirements  
✅ Complete information preservation (text + visuals)  
✅ Searchable, navigable document structure  
✅ Team collaboration and reference  
✅ Compliance verification and tracking  

The tool directly supports the #Prompt2Code2 competition's goal of demonstrating effective AI-assisted development by making requirements accessible to AI coding assistants.

---

**Author:** Created for HackYeah 2025 #Prompt2Code2 Competition  
**Last Updated:** October 4, 2025  
**License:** Project-specific tool for UKNF competition requirements
