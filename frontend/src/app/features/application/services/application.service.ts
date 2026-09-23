import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ApplicationStatus,
  CreateJobApplication,
  CursorPagedResult,
  JobApplication,
  JobApplicationDetail,
  StatusHistoryEntry,
  UpdateJobApplication,
  UpdateStatusRequest
} from '../../../shared/features/application/models/application.model';
import { ApplicationConfig } from '../configs/application.config';

export interface ApplicationFilters {
  status?: ApplicationStatus | null;
  search?: string | null;
  tag?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
  cursor?: string | null;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  constructor(private http: HttpClient) {}

  getApplications(filters: ApplicationFilters = {}): Observable<CursorPagedResult<JobApplication>> {
    let params = new HttpParams();
    if (filters.status !== undefined && filters.status !== null) {
      params = params.set('status', filters.status);
    }
    if (filters.search) params = params.set('search', filters.search);
    if (filters.tag) params = params.set('tag', filters.tag);
    if (filters.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters.toDate) params = params.set('toDate', filters.toDate);
    if (filters.cursor) params = params.set('cursor', filters.cursor);

    params = params.set('pageSize', filters.pageSize ?? 20);

    return this.http.get<CursorPagedResult<JobApplication>>(ApplicationConfig.LIST, { params });
  }

  getApplication(id: number): Observable<JobApplicationDetail> {
    return this.http.get<JobApplicationDetail>(ApplicationConfig.ITEM(id));
  }

  createApplication(dto: CreateJobApplication): Observable<JobApplication> {
    return this.http.post<JobApplication>(ApplicationConfig.LIST, dto);
  }

  updateApplication(id: number, dto: UpdateJobApplication): Observable<void> {
    return this.http.put<void>(ApplicationConfig.ITEM(id), dto);
  }

  deleteApplication(id: number): Observable<void> {
    return this.http.delete<void>(ApplicationConfig.ITEM(id));
  }

  updateStatus(id: number, dto: UpdateStatusRequest): Observable<void> {
    return this.http.patch<void>(ApplicationConfig.STATUS(id), dto);
  }

  getHistory(id: number): Observable<StatusHistoryEntry[]> {
    return this.http.get<StatusHistoryEntry[]>(ApplicationConfig.HISTORY(id));
  }
}
