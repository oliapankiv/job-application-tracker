import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import {
  ApplicationStatus,
  CreateJobApplication,
  JobApplication,
  JobApplicationDetail,
  PagedResult,
  StatusHistoryEntry,
  UpdateJobApplication,
  UpdateStatusRequest
} from '../../../models/application.model';
import { ApplicationsConfig } from '../configs/application.config';

export interface ApplicationFilters {
  status?: ApplicationStatus | null;
  search?: string | null;
  tag?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly baseUrl = `${environment.apiUrl}${ApplicationsConfig.BASE}`;

  constructor(private http: HttpClient) {}

  getApplications(filters: ApplicationFilters = {}): Observable<PagedResult<JobApplication>> {
    let params = new HttpParams();
    if (filters.status !== undefined && filters.status !== null) {
      params = params.set('status', filters.status);
    }
    if (filters.search) params = params.set('search', filters.search);
    if (filters.tag) params = params.set('tag', filters.tag);
    if (filters.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters.toDate) params = params.set('toDate', filters.toDate);

    params = params.set('page', filters.page ?? 1);
    params = params.set('pageSize', filters.pageSize ?? 100);

    return this.http.get<PagedResult<JobApplication>>(this.baseUrl, { params });
  }

  getApplication(id: number): Observable<JobApplicationDetail> {
    return this.http.get<JobApplicationDetail>(`${this.baseUrl}/${id}`);
  }

  createApplication(dto: CreateJobApplication): Observable<JobApplication> {
    return this.http.post<JobApplication>(this.baseUrl, dto);
  }

  updateApplication(id: number, dto: UpdateJobApplication): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  deleteApplication(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  updateStatus(id: number, dto: UpdateStatusRequest): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, dto);
  }

  getHistory(id: number): Observable<StatusHistoryEntry[]> {
    return this.http.get<StatusHistoryEntry[]>(`${this.baseUrl}/${id}/history`);
  }
}
