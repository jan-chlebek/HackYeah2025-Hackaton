import plotly.graph_objects as go
import pandas as pd

# Create the data
data = [
  {"Color_Name": "Primary Blue", "Hex": "#003366", "RGB": "0, 51, 102", "Usage": "Headers, Navigation, Buttons", "WCAG_Compliant": "Yes (with white text)"},
  {"Color_Name": "Accent Blue", "Hex": "#0073E6", "RGB": "0, 115, 230", "Usage": "Links, Highlights", "WCAG_Compliant": "Yes (with white text)"},
  {"Color_Name": "Light Blue", "Hex": "#E6F3FF", "RGB": "230, 243, 255", "Usage": "Backgrounds, Sections", "WCAG_Compliant": "Yes (with dark text)"},
  {"Color_Name": "White", "Hex": "#FFFFFF", "RGB": "255, 255, 255", "Usage": "Main Background", "WCAG_Compliant": "Yes"},
  {"Color_Name": "Light Gray", "Hex": "#F5F5F5", "RGB": "245, 245, 245", "Usage": "Section Backgrounds", "WCAG_Compliant": "Yes (with dark text)"},
  {"Color_Name": "Medium Gray", "Hex": "#666666", "RGB": "102, 102, 102", "Usage": "Secondary Text", "WCAG_Compliant": "Yes (on white background)"},
  {"Color_Name": "Dark Gray", "Hex": "#333333", "RGB": "51, 51, 51", "Usage": "Primary Text", "WCAG_Compliant": "Yes (on light backgrounds)"}
]

df = pd.DataFrame(data)

# Create abbreviated versions for display (15 char limit)
df['Short_Name'] = df['Color_Name'].apply(lambda x: x[:15] if len(x) > 15 else x)
df['Short_Usage'] = df['Usage'].apply(lambda x: x[:15] + '...' if len(x) > 15 else x)

# Function to determine text color based on background
def get_text_color(hex_color):
    # Convert hex to RGB
    hex_color = hex_color.lstrip('#')
    r, g, b = tuple(int(hex_color[i:i+2], 16) for i in (0, 2, 4))
    # Calculate luminance
    luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255
    return 'black' if luminance > 0.7 else 'white'

# Create the figure
fig = go.Figure()

# Add color swatches as rectangles
for i, row in df.iterrows():
    y_pos = len(df) - i - 1  # Reverse order for top-to-bottom display
    
    # Add color swatch
    fig.add_shape(
        type="rect",
        x0=0, x1=2,
        y0=y_pos-0.4, y1=y_pos+0.4,
        fillcolor=row['Hex'],
        line=dict(color='black', width=1)
    )
    
    # Determine text color for contrast
    text_color = get_text_color(row['Hex'])
    
    # Add color name on the swatch
    fig.add_annotation(
        x=1, y=y_pos,
        text=row['Short_Name'],
        showarrow=False,
        font=dict(color=text_color, size=12, family="Arial Black"),
        xanchor="center",
        yanchor="middle"
    )
    
    # Add hex code to the right of swatch
    fig.add_annotation(
        x=2.3, y=y_pos + 0.1,
        text=row['Hex'],
        showarrow=False,
        font=dict(color='black', size=11),
        xanchor="left",
        yanchor="middle"
    )
    
    # Add usage description below hex code
    fig.add_annotation(
        x=2.3, y=y_pos - 0.1,
        text=row['Short_Usage'],
        showarrow=False,
        font=dict(color='gray', size=10),
        xanchor="left",
        yanchor="middle"
    )

# Update layout
fig.update_layout(
    title="KNF Color Palette",
    xaxis=dict(
        range=[-0.5, 5],
        showticklabels=False,
        showgrid=False,
        zeroline=False,
        visible=False
    ),
    yaxis=dict(
        range=[-0.5, len(df)-0.5],
        showticklabels=False,
        showgrid=False,
        zeroline=False,
        visible=False
    ),
    plot_bgcolor='white',
    height=500,
    showlegend=False
)

# Save the chart as both PNG and SVG
fig.write_image("color_palette.png")
fig.write_image("color_palette.svg", format="svg")

print("Updated color palette chart created and saved!")