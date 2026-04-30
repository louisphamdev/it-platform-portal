import { apiClient } from './api';

export interface AuditLog {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  tenantId: string;
  userId: string;
  userName: string;
  userRole: string;
  ipAddress: string;
  userAgent: string;
  requestData: string;
  responseData: string;
  statusCode: number;
  errorMessage: string;
  durationMs: number;
  createdAt: string;
}

export interface AuditQuery {
  tenantId?: string;
  userId?: string;
  action?: string;
  entityType?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
}

export const auditService = {
  async query(params: AuditQuery): Promise<AuditLog[]> {
    const response = await apiClient.get('/audit', { params });
    return response.data;
  },

  async getById(id: string): Promise<AuditLog> {
    const response = await apiClient.get(`/audit/${id}`);
    return response.data;
  },

  async getCount(params: AuditQuery): Promise<number> {
    const response = await apiClient.get('/audit/count', { params });
    return response.data;
  },
};
