# Prompt

Remove background from this component. Make register and forgot password blocked (it's only mock). Login button should accept any values

# Response

I've successfully made the following changes to the login component:

## 1. Removed Background
- **File**: `login.component.css`
- Removed the gradient background (`background: linear-gradient(135deg, #003366 0%, #0073E6 100%);`) from `.login-page`
- The page now has a transparent/default background

## 2. Blocked Register and Forgot Password Links
- **Files**: `login.component.html` and `login.component.css`
- Changed both links from clickable `<a>` elements to disabled `<span>` elements
- Added "(MOCK)" labels to both links to clearly indicate they're non-functional
- Applied disabled styles:
  - Gray color (#999999)
  - Reduced opacity (0.6)
  - `pointer-events: none` to prevent any interaction
  - `cursor: not-allowed` for visual feedback

## 3. Made Login Button Accept Any Values
- **File**: `login.component.ts`
- Removed all form validators from the email and password fields
- Removed validation check in `onSubmit()` method
- The form now accepts any input (including empty values) and will attempt to submit

## Changes Summary

### Modified Files:
1. **login.component.css**
   - Removed background gradient from `.login-page`
   - Disabled `.forgot-password` link styling
   - Disabled `.register-link` styling

2. **login.component.html**
   - Changed forgot password from `<a routerLink>` to `<span>` with "(MOCK)" label
   - Changed register link from `<a routerLink>` to `<span>` with "(MOCK)" label

3. **login.component.ts**
   - Removed `Validators.required` and `Validators.email` from email field
   - Removed `Validators.required` and `Validators.minLength(6)` from password field
   - Removed form validation check in `onSubmit()` method

The login component is now a mock implementation that accepts any input values and clearly indicates that registration and password reset features are not available.
