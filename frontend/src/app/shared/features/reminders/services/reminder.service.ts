import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateReminder, Reminder, UpdateReminder } from '../../../../features/applications/models/application.model';

@Injectable({ providedIn: 'root' })
export class ReminderService {
  constructor(private http: HttpClient) {}

  getUpcoming(): Observable<Reminder[]> {
    return this.http.get<Reminder[]>('/reminders/upcoming');
  }

  createReminder(applicationId: number, dto: CreateReminder): Observable<Reminder> {
    return this.http.post<Reminder>(`/applications/${applicationId}/reminders`, dto);
  }

  updateReminder(reminderId: number, dto: UpdateReminder): Observable<void> {
    return this.http.put<void>(`/reminders/${reminderId}`, dto);
  }

  deleteReminder(reminderId: number): Observable<void> {
    return this.http.delete<void>(`/reminders/${reminderId}`);
  }
}
