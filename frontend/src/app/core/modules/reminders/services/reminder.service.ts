import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CreateReminder, Reminder, UpdateReminder } from '../../../models/application.model';

@Injectable({ providedIn: 'root' })
export class ReminderService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUpcoming(): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(`${this.baseUrl}/reminders/upcoming`);
  }

  createReminder(applicationId: number, dto: CreateReminder): Observable<Reminder> {
    return this.http.post<Reminder>(`${this.baseUrl}/applications/${applicationId}/reminders`, dto);
  }

  updateReminder(reminderId: number, dto: UpdateReminder): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/reminders/${reminderId}`, dto);
  }

  deleteReminder(reminderId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/reminders/${reminderId}`);
  }
}
