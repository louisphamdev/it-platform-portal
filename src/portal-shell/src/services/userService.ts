import { apiClient } from './api';

export interface UserProfile {
  id: string;
  userId: string;
  department?: string;
  jobTitle?: string;
  address?: string;
  city?: string;
  country?: string;
  avatarUrl?: string;
  dateOfBirth?: string;
}

export interface UpdateProfileRequest {
  department?: string;
  jobTitle?: string;
  address?: string;
  city?: string;
  country?: string;
  avatarUrl?: string;
  dateOfBirth?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export const userService = {
  async getProfile(userId: string): Promise<UserProfile> {
    const response = await apiClient.get(`/users/${userId}/profile`);
    return response.data;
  },

  async updateProfile(userId: string, data: UpdateProfileRequest): Promise<UserProfile> {
    const response = await apiClient.put(`/users/${userId}/profile`, data);
    return response.data;
  },

  async changePassword(userId: string, data: ChangePasswordRequest): Promise<void> {
    await apiClient.post(`/users/${userId}/change-password`, data);
  },

  async revokeAllSessions(userId: string): Promise<void> {
    await apiClient.delete(`/users/${userId}/sessions`);
  },
};
