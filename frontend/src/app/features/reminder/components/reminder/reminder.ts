import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReminderService } from '../../../../shared/features/reminder/services/reminder.service';
import { Reminder as ReminderItem } from '../../../../shared/features/reminder/models/reminder.model';
import { ApplicationRoute } from '../../../../shared/features/application/routes/application.route';

@Component({
  selector: 'app-reminder',
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
  templateUrl: './reminder.html',
  styleUrl: './reminder.scss'
})
export class Reminder implements OnInit {
  readonly loading = signal(true);
  readonly reminders = signal<ReminderItem[]>([]);

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

  complete(reminder: ReminderItem): void {
    this.reminderService
      .updateReminder(reminder.id, { dueDate: reminder.dueDate, message: reminder.message, isCompleted: true })
      .subscribe(() => this.load());
  }

  remove(reminder: ReminderItem): void {
    this.reminderService.deleteReminder(reminder.id).subscribe(() => this.load());
  }

  goToApplication(reminder: ReminderItem): void {
    this.router.navigate([ApplicationRoute.Detail(reminder.applicationId)]);
  }
}
