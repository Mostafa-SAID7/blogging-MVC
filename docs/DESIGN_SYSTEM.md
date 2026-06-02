# BloggingAgent - Centralized Design System

## Overview

This document describes the comprehensive, centralized CSS design system implemented for the BloggingAgent application. The system uses a dark orange theme with professional styling, responsive design, and accessibility considerations.

## Design Philosophy

- **Single Source of Truth**: All colors, spacing, shadows, and animations defined in CSS custom properties (variables)
- **Consistency**: No duplicate styling - all components use centralized utilities
- **Professionalism**: Gradient backgrounds, smooth transitions, and thoughtful spacing
- **Accessibility**: Dark mode support, reduced motion preferences, high contrast modes
- **Responsiveness**: Mobile-first approach with breakpoints at 768px and 576px
- **Performance**: Optimized animations and transitions

## Color Palette

### Primary Orange Theme
```
Dark Orange:        #D97706  (--color-primary-dark)
Bright Orange:      #F59E0B  (--color-primary)
Light Orange:       #FCD34D  (--color-primary-light)
```

### Neutrals
```
Very Dark Gray:     #111827  (--color-gray-900)
Dark Gray:          #1F2937  (--color-gray-800)
Medium Gray:        #374151  (--color-gray-700)
Light Gray:         #F3F4F6  (--color-gray-100)
White:              #FFFFFF  (--color-light)
```

### Status Colors
```
Success:            #10B981  (--color-success)
Danger:             #EF4444  (--color-danger)
Warning:            #F59E0B  (--color-warning)
Info:               #3B82F6  (--color-info)
```

## Spacing System

Centralized spacing scale used throughout:

```
xs:   0.25rem  (4px)
sm:   0.5rem   (8px)
md:   1rem     (16px)
lg:   1.5rem   (24px)
xl:   2rem     (32px)
2xl:  3rem     (48px)
3xl:  4rem     (64px)
```

## Border Radius Scale

```
sm:    0.375rem  (6px)
md:    0.5rem    (8px)
lg:    0.75rem   (12px)
xl:    1rem      (16px)
2xl:   1.5rem    (24px)
full:  9999px    (fully rounded)
```

## Shadow System

Professional shadow system with varying depths:

```
xs:       0 1px 2px 0 rgba(0,0,0,0.05)
sm:       0 1px 2px 0 rgba(0,0,0,0.1)
md:       0 4px 6px -1px rgba(0,0,0,0.1)
lg:       0 10px 15px -3px rgba(0,0,0,0.1)
xl:       0 20px 25px -5px rgba(0,0,0,0.1)
2xl:      0 25px 50px -12px rgba(0,0,0,0.25)
orange:   0 4px 20px rgba(217,119,6,0.15)  (theme-specific)
```

## Animation & Transitions

Smooth, professional transitions with three speed options:

```
Fast:     150ms cubic-bezier(0.4, 0, 0.2, 1)
Base:     250ms cubic-bezier(0.4, 0, 0.2, 1)
Slow:     350ms cubic-bezier(0.4, 0, 0.2, 1)
```

### Keyframe Animations
- **slideDown**: Dropdown menu appearance (used in navbar)
- **slideIn**: Alert notifications appearance
- **pulse**: Notification badge animation

## Component Styling

### Navigation Bar
- Gradient background (dark gray 900 to 800)
- Orange bottom border (3px)
- Hover effects with underline animation
- Responsive dropdown menus
- Animated notification badge

### Cards
- White background with rounded corners (lg)
- Medium shadow by default
- Hover lift effect (translateY -4px) with orange shadow
- Gradient headers (dark gray 800 to 700)
- Orange bottom border on headers

### Buttons
- Primary: Orange gradient with shadow
- Hover: Lighter gradient with orange shadow
- Secondary: Transparent with orange border
- All buttons use flex layout for icon + text
- Focus state: Custom orange outline

### Forms
- 2px borders (gray-300)
- Focus: Orange border with subtle orange shadow
- Rounded corners (md)
- Large padding for mobile usability
- Floating labels with orange accent on focus

### Alerts
- Flex layout with icon support
- Colored left border (4px)
- Semi-transparent background
- Slide-in animation
- Icons aligned to top

### Tables
- Professional styling with orange accents
- 2px orange borders on thead and last row
- Hover rows with light orange background
- Proper vertical alignment

## Utility Classes

### Spacing
- `mt-xs`, `mt-sm`, `mt-md`, `mt-lg`, `mt-xl` - Margin top
- `mb-xs`, `mb-sm`, `mb-md`, `mb-lg`, `mb-xl` - Margin bottom
- `p-xs`, `p-sm`, `p-md`, `p-lg`, `p-xl` - Padding

### Shadows
- `shadow-xs` through `shadow-2xl` - Various shadow depths
- `shadow-orange` - Theme-specific orange shadow

### Rounded Corners
- `rounded-sm` through `rounded-2xl` - Various border radius values
- `rounded-full` - Fully rounded (9999px)

### Text Colors
- `text-primary` - Dark orange
- `text-success`, `text-danger`, `text-warning`, `text-info`
- `text-muted` - Gray-600

### Flex Layout
- `d-flex` - Flexbox display
- `align-items-center` - Center vertical alignment
- `justify-content-center`, `justify-content-between`
- `gap-xs`, `gap-sm`, `gap-md`, `gap-lg` - Gap between flex items

### Transitions
- `transition-fast`, `transition-base`, `transition-slow`

### Hover Effects
- `hover-lift` - Translate up + shadow
- `hover-scale` - Scale 1.05x
- `hover-glow` - Orange shadow only

## Scrollbar Styling

Custom scrollbar for all modern browsers:

**Webkit (Chrome, Safari, Edge)**
- Track: Light gray
- Thumb: Orange gradient (dark to bright)
- Hover: Lighter orange gradient
- Rounded corners (full)

**Firefox**
- Color: Dark orange thumb on gray track
- Width: Thin (10px)

## Responsive Design

### Breakpoints
- **768px**: Tablet and up
  - Larger headings
  - Optimized navbar
  - Better card spacing

- **576px**: Mobile devices
  - Smaller headings
  - Reduced padding on cards
  - Stack flex items

### Mobile-First
- Base styles apply to all devices
- Breakpoints progressively enhance for larger screens
- Touch-friendly button sizes
- Readable text at all sizes

## Dark Mode Support

Automatically adapts to system dark mode preference (`prefers-color-scheme: dark`):

- Adjusted gray palette for dark backgrounds
- Maintained orange theme for consistency
- Better contrast in dark environments
- Automatic CSS variable override

## Accessibility Features

### Reduced Motion
- Respects `prefers-reduced-motion: reduce`
- Removes animations when user prefers
- Maintains functionality with no motion

### High Contrast
- Respects `prefers-contrast: more`
- Adds borders to elements
- Increased color contrast

### Keyboard Navigation
- Focus states clearly visible (orange outline)
- Tab order preserved
- All interactive elements accessible

## Typography

- **Font Family**: 'Segoe UI', Tahoma, Geneva, Verdana
- **Monospace**: 'SFMono-Regular', Menlo
- **Line Height**: 1.6 for body text, 1.2 for headings
- **Font Weight**: 600 for labels, 700 for headings

## Best Practices

### When Adding New Components

1. **Use existing variables** for colors, spacing, shadows
   ```css
   background-color: var(--color-primary-dark);
   padding: var(--spacing-lg);
   box-shadow: var(--shadow-md);
   ```

2. **Maintain consistency** - no hard-coded colors
   ```css
   /* ❌ Avoid */
   color: #FF9900;
   
   /* ✅ Use */
   color: var(--color-primary);
   ```

3. **Use utility classes** before writing custom CSS
   ```html
   <!-- ✅ Prefer utilities -->
   <div class="shadow-lg rounded-lg p-lg mb-xl">
   
   <!-- ❌ Avoid inline styles -->
   <div style="box-shadow: 0...;padding: 24px;">
   ```

4. **Responsive approach** - mobile first
   ```css
   /* Base mobile styles */
   font-size: 1rem;
   
   /* Scale up for larger devices */
   @media (min-width: 768px) {
     font-size: 1.25rem;
   }
   ```

5. **Animations should be fast** (< 300ms) for better UX
   ```css
   transition: all var(--transition-base);  /* 250ms */
   ```

## File Structure

- `/wwwroot/css/site.css` - Main design system file (containing all CSS)
- `/wwwroot/favicon.svg` - Orange-themed icon
- All Bootstrap classes still available for quick layouts

## Browser Support

- Modern browsers with CSS custom properties support
- Chrome 49+, Firefox 31+, Safari 9.1+, Edge 15+
- Graceful degradation for older browsers

## Customization

To customize the theme, edit CSS custom properties in `:root`:

```css
:root {
  --color-primary-dark: #D97706;  /* Change primary color */
  --spacing-md: 1rem;              /* Adjust base spacing */
  --shadow-md: /* Your shadow */;  /* Customize shadows */
}
```

## Performance

- Zero runtime overhead (pure CSS variables)
- Minimal file size impact (< 30KB)
- No CSS-in-JS libraries required
- Browser-native performance

---

**Last Updated**: June 2026
**Version**: 1.0
**Theme**: Dark Orange Professional
