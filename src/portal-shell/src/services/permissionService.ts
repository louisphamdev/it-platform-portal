import { apiClient } from './api';

export interface Permission {
  id: string;
  code: string;
  name: string;
  description: string;
  module: string;
  isActive: boolean;
}

export const permissionService = {
  async getAll(): Promise<Permission[]> {
    const response = await apiClient.get('/permissions');
    return response.data;
  },

  async getByRole(roleId: string): Promise<Permission[]> {
    const response = await apiClient.get(`/permissions/role/${roleId}`);
    return response.data;
  },

  async getByUser(userId: string): Promise<string[]> {
    const response = await apiClient.get(`/permissions/user/${userId}`);
    return response.data;
  },

  async assignPermissions(roleId: string, permissionIds: string[]): Promise<void> {
    await apiClient.post('/permissions/assign', { roleId, permissionIds });
  },

  async removePermission(roleId: string, permissionId: string): Promise<void> {
    await apiClient.delete(`/permissions/role/${roleId}/permission/${permissionId}`);
  },
};
