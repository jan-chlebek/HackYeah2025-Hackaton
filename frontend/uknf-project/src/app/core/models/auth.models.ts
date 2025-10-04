/**
 * Authentication and authorization models
 * Generated DTOs matching backend API contracts
 */

/**
 * Login request payload
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Login response from API
 */
export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserInfo;
}

/**
 * User information included in login response
 */
export interface UserInfo {
  id: number;
  email: string;
  fullName: string;
  roles: string[];
  permissions: string[];
  supervisedEntityId?: number;
}

/**
 * Refresh token request
 */
export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

/**
 * Token revocation request
 */
export interface RevokeTokenRequest {
  refreshToken: string;
  reason?: string;
}

/**
 * Change password request
 */
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
