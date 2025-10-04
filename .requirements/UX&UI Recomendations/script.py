# Create a comprehensive analysis of KNF color schemes and UI/UX patterns
# Let's organize the findings into a structured format

import pandas as pd

# KNF Color Scheme Analysis based on research
knf_color_analysis = {
    "Primary Colors": {
        "Dark Blue": {
            "hex": "#003366",  # Based on web safe alternative and building signage
            "rgb": "rgb(0, 51, 102)",
            "usage": "Primary brand color, headers, navigation, official signage",
            "description": "Deep navy blue used consistently across KNF branding"
        },
        "White": {
            "hex": "#FFFFFF",
            "rgb": "rgb(255, 255, 255)",
            "usage": "Background, text on dark elements, clean layouts",
            "description": "Primary background and contrast color"
        },
        "Light Blue": {
            "hex": "#0AB68B", # Teal-like blue found in modern financial sites
            "rgb": "rgb(10, 182, 139)",
            "usage": "Accent color, links, interactive elements",
            "description": "Secondary blue for highlights and accents"
        }
    },
    "Secondary Colors": {
        "Light Gray": {
            "hex": "#F5F5F5",
            "rgb": "rgb(245, 245, 245)",
            "usage": "Section backgrounds, subtle separators",
            "description": "Neutral background for content sections"
        },
        "Dark Gray": {
            "hex": "#333333",
            "rgb": "rgb(51, 51, 51)",
            "usage": "Body text, secondary text elements",
            "description": "Primary text color for readability"
        },
        "Medium Gray": {
            "hex": "#666666",
            "rgb": "rgb(102, 102, 102)",
            "usage": "Subtitles, captions, less important text",
            "description": "Secondary text color"
        }
    }
}

# UI/UX Design Patterns observed from KNF research
knf_ui_patterns = {
    "Navigation": {
        "Type": "Horizontal top navigation with dropdown menus",
        "Style": "Clean, minimalist with clear hierarchy",
        "Colors": "Dark blue background with white text",
        "Features": ["Multi-level dropdown", "Search functionality", "Language switcher (EN/PL)"]
    },
    "Typography": {
        "Primary_Font": "Clean sans-serif (similar to Arial, Lato, or Roboto)",
        "Hierarchy": {
            "H1": "Large, bold, dark blue or dark gray",
            "H2": "Medium, semi-bold",
            "H3": "Regular weight, slightly smaller",
            "Body": "Regular weight, dark gray (#333)"
        },
        "Line_Height": "1.4-1.6 for optimal readability",
        "Font_Sizes": "14-16px for body text, scaling up for headers"
    },
    "Layout": {
        "Structure": "Clean grid-based layout",
        "Spacing": "Generous white space for readability",
        "Cards": "Subtle shadows, rounded corners for content blocks",
        "Buttons": "Rounded corners, blue primary buttons with white text"
    },
    "Accessibility": {
        "Contrast": "WCAG AA compliant (4.5:1 minimum ratio)",
        "Font_Size": "Adjustable font sizing options",
        "Colors": "Not solely relied upon for information",
        "Navigation": "Keyboard accessible, logical tab order"
    }
}

# Government Website Best Practices
gov_best_practices = {
    "Color_Guidelines": [
        "High contrast ratios (4.5:1 minimum for WCAG AA)",
        "Professional color palette with blues, grays, whites",
        "Limited color palette (3-5 main colors)",
        "Accessible to colorblind users",
        "Cultural appropriateness for Polish audience"
    ],
    "Typography_Guidelines": [
        "Sans-serif fonts for web (Arial, Lato, Roboto)",
        "Clear hierarchy with consistent sizing",
        "Readable font sizes (14px minimum)",
        "Good line height (1.4-1.6)",
        "Limited font families (1-2 maximum)"
    ],
    "UI_Elements": [
        "Clean, minimal design",
        "Consistent button styles",
        "Clear navigation structure",
        "Search functionality",
        "Responsive design",
        "Fast loading times"
    ]
}

# Create color palette recommendations
color_palette_recommendations = pd.DataFrame([
    {"Color_Name": "Primary Blue", "Hex": "#003366", "RGB": "0, 51, 102", "Usage": "Headers, Navigation, Buttons", "WCAG_Compliant": "Yes (with white text)"},
    {"Color_Name": "Accent Blue", "Hex": "#0073E6", "RGB": "0, 115, 230", "Usage": "Links, Highlights", "WCAG_Compliant": "Yes (with white text)"},
    {"Color_Name": "Light Blue", "Hex": "#E6F3FF", "RGB": "230, 243, 255", "Usage": "Backgrounds, Sections", "WCAG_Compliant": "Yes (with dark text)"},
    {"Color_Name": "White", "Hex": "#FFFFFF", "RGB": "255, 255, 255", "Usage": "Main Background", "WCAG_Compliant": "Yes"},
    {"Color_Name": "Light Gray", "Hex": "#F5F5F5", "RGB": "245, 245, 245", "Usage": "Section Backgrounds", "WCAG_Compliant": "Yes (with dark text)"},
    {"Color_Name": "Medium Gray", "Hex": "#666666", "RGB": "102, 102, 102", "Usage": "Secondary Text", "WCAG_Compliant": "Yes (on white background)"},
    {"Color_Name": "Dark Gray", "Hex": "#333333", "RGB": "51, 51, 51", "Usage": "Primary Text", "WCAG_Compliant": "Yes (on light backgrounds)"}
])

print("KNF Color Palette Recommendations:")
print("="*50)
print(color_palette_recommendations.to_string(index=False))

# Save to CSV for user
color_palette_recommendations.to_csv('knf_color_palette_recommendations.csv', index=False)

print("\n\nColor palette saved to 'knf_color_palette_recommendations.csv'")