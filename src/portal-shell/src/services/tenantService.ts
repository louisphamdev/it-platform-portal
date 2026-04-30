import { apiClient } from './api';

export interface Tenant {
  id: string;
  name: string;
  code: string;
  status: 'Active' | 'Suspended' | 'Inactive';
  createdAt: string;
  updatedAt: string;
}

export interface CreateTenantRequest {
  name: string;
  code: string;
}

export interface UpdateTenantRequest {
  name?: string;
  description?: string;
}

export const tenantService = {
  async getAll(): Promise<Tenant[]> {
    const response = await apiClient.get('/tenants');
    return response.data;
  },

  async getById(id: string): Promise<Tenant> {
    const response = await apiClient.get(`/tenants/${id}`);
    return response.data;
  },

  async create(data: CreateTenantRequest): Promise<Tenant> {
    const response = await apiClient.post('/tenants', data);
    return response.data;
  },

  async update(id: string, data: UpdateTenantRequest): Promise<Tenant> {
    const response = await apiClient.put(`/tenants/${id}`, data);
    return response.data;
  },

  async suspend(id: string): Promise<void> {
    await apiClient.post(`/tenants/${id}/suspend`);
  },

  async activate(id: string): Promise<void> {
    await apiClient.post(`/tenants/${id}/activate`);
  },

  async getUserCount(id: string): Promise<number> {
    const response = await apiClient.get(`/tenants/${id}/user-count`);
    return response.data;
  },
};
