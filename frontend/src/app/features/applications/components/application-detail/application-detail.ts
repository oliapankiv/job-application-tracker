import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { ApplicationService } from '../../../../core/modules/applications/services/application.service';
import { ContactService } from '../../../../core/modules/contacts/services/contact.service';
import { ReminderService } from '../../../../core/modules/reminders/services/reminder.service';
import {
  ApplicationStatus,
  JobApplicationDetail,
  SOURCE_LABELS,
  STATUS_LABELS,
  WORK_TYPE_LABELS
} from '../../../../core/models/application.model';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { ApplicationsRoute } from '../../../../shared/features/applications/routes/applications.route';
import { ApplicationForm } from '../application-form/application-form';
import { ContactFormDialog } from '../contact-form-dialog/contact-form-dialog';
import { ReminderFormDialog } from '../reminder-form-dialog/reminder-form-dialog';

@Component({
  selector: 'app-application-detail',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatTooltipModule,
    StatusBadge
  ],
  templateUrl: './application-detail.html',
  styleUrl: './application-detail.scss'
})
export class ApplicationDetail implements OnInit {
  readonly applicationsRoute = ApplicationsRoute.Main;
  readonly loading = signal(true);
  readonly application = signal<JobApplicationDetail | null>(null);
  readonly statusUpdating = signal(false);
  selectedStatus: ApplicationStatus | null = null;

  readonly statusOptions = Object.entries(STATUS_LABELS).map(([value, label]) => ({
    value: Number(value) as ApplicationStatus,
    label
  }));

  readonly sourceLabels = SOURCE_LABELS;
  readonly workTypeLabels = WORK_TYPE_LABELS;

  private applicationId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private applicationService: ApplicationService,
    private contactService: ContactService,
    private reminderService: ReminderService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.applicationId = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.applicationService.getApplication(this.applicationId).subscribe({
      next: (app) => {
        this.application.set(app);
        this.selectedStatus = app.status;
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.router.navigate([ApplicationsRoute.Main]);
      }
    });
  }

  updateStatus(): void {
    if (this.selectedStatus === null || this.selectedStatus === this.application()?.status) return;

    this.statusUpdating.set(true);
    this.applicationService.updateStatus(this.applicationId, { status: this.selectedStatus }).subscribe(() => {
      this.statusUpdating.set(false);
      this.load();
    });
  }

  openEditDialog(): void {
    const app = this.application();
    if (!app) return;

    const ref = this.dialog.open(ApplicationForm, { data: { application: app }, width: '520px' });
    ref.afterClosed().subscribe((updated) => {
      if (updated) this.load();
    });
  }

  deleteApplication(): void {
    const app = this.application();
    if (!app) return;

    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete application',
        message: `Delete the application to ${app.companyName} for ${app.jobTitle}? This cannot be undone.`,
        confirmLabel: 'Delete'
      }
    });

    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.applicationService.deleteApplication(app.id).subscribe(() => this.router.navigate([ApplicationsRoute.Main]));
      }
    });
  }

  openAddContact(): void {
    const ref = this.dialog.open(ContactFormDialog, { width: '420px' });
    ref.afterClosed().subscribe((contact) => {
      if (contact) {
        this.contactService.createContact(this.applicationId, contact).subscribe(() => this.load());
      }
    });
  }

  openAddReminder(): void {
    const ref = this.dialog.open(ReminderFormDialog, { width: '420px' });
    ref.afterClosed().subscribe((reminder) => {
      if (reminder) {
        this.reminderService.createReminder(this.applicationId, reminder).subscribe(() => this.load());
      }
    });
  }

  toggleReminderComplete(reminderId: number, isCompleted: boolean, dueDate: string, message: string): void {
    this.reminderService.updateReminder(reminderId, { dueDate, message, isCompleted: !isCompleted }).subscribe(() => this.load());
  }

  deleteReminder(reminderId: number): void {
    this.reminderService.deleteReminder(reminderId).subscribe(() => this.load());
  }

  deleteContact(contactId: number): void {
    this.contactService.deleteContact(contactId).subscribe(() => this.load());
  }
}
