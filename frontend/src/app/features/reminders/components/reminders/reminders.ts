import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReminderService } from '../../../../shared/features/reminders/services/reminder.service';
import { Reminder } from '../../../applications/models/application.model';
import { ApplicationsRoute } from '../../../../shared/features/applications/routes/applications.route';

@Component({
  selector: 'app-reminders',
  standalone: true,
  imports: [
    DatePipe,
    MatCardModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './reminders.html',
  styleUrl: './reminders.scss'
})
export class Reminders implements OnInit {
  readonly loading = signal(true);
  readonly reminders = signal<Reminder[]>([]);

  constructor(private reminderService: ReminderService, private router: Router) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.reminderService.getUpcoming().subscribe((reminders) => {
      this.reminders.set(reminders);
      this.loading.set(false);
    });
  }

  isOverdue(dueDate: string): boolean {
    return new Date(dueDate) < new Date();
  }

  complete(reminder: Reminder): void {
    this.reminderService
      .updateReminder(reminder.id, { dueDate: reminder.dueDate, message: reminder.message, isCompleted: true })
      .subscribe(() => this.load());
  }

  remove(reminder: Reminder): void {
    this.reminderService.deleteReminder(reminder.id).subscribe(() => this.load());
  }

  goToApplication(reminder: Reminder): void {
    this.router.navigate([ApplicationsRoute.Detail(reminder.applicationId)]);
  }
}
