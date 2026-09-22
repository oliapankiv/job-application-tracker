import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateReminder, Reminder, UpdateReminder } from '../../../../shared/features/reminder/models/reminder.model';
import { ReminderConfig } from '../configs/reminder.config';

@Injectable({ providedIn: 'root' })
export class ReminderService {
  constructor(private http: HttpClient) {}

  getUpcoming(): Observable<Reminder[]> {
    return this.http.get<Reminder[]>(ReminderConfig.UPCOMING);
  }

  createReminder(applicationId: number, dto: CreateReminder): Observable<Reminder> {
    return this.http.post<Reminder>(ReminderConfig.CREATE(applicationId), dto);
  }

  updateReminder(reminderId: number, dto: UpdateReminder): Observable<void> {
    return this.http.put<void>(ReminderConfig.ITEM(reminderId), dto);
  }

  deleteReminder(reminderId: number): Observable<void> {
    return this.http.delete<void>(ReminderConfig.ITEM(reminderId));
  }
}
