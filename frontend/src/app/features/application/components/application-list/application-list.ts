import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { debounceTime } from 'rxjs';
import { ApplicationService } from '../../services/application.service';
import { ApplicationStatus, JobApplication, STATUS_LABELS } from '../../../../shared/features/application/models/application.model';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { ApplicationForm } from '../application-form/application-form';
import { ApplicationRoute } from '../../../../shared/features/application/routes/application.route';

@Component({
  selector: 'app-application-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    StatusBadge,
    DatePipe
  ],
  templateUrl: './application-list.html',
  styleUrl: './application-list.scss'
})
export class ApplicationList implements OnInit {
  private fb = inject(FormBuilder);

  readonly loading = signal(true);
  readonly applications = signal<JobApplication[]>([]);
  readonly displayedColumns = ['companyName', 'jobTitle', 'status', 'workType', 'appliedDate', 'actions'];

  readonly statusOptions = Object.entries(STATUS_LABELS).map(([value, label]) => ({
    value: Number(value) as ApplicationStatus,
    label
  }));

  readonly filterForm = this.fb.group({
    search: [''],
    status: [null as ApplicationStatus | null]
  });

  constructor(
    private applicationService: ApplicationService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.load();

    this.filterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => this.load());
  }

  load(): void {
    this.loading.set(true);
    const { search, status } = this.filterForm.getRawValue();

    this.applicationService.getApplications({ search: search || null, status }).subscribe((result) => {
      this.applications.set(result.items);
      this.loading.set(false);
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(ApplicationForm, { data: {}, width: '520px' });
    ref.afterClosed().subscribe((created) => {
      if (created) this.load();
    });
  }

  openEditDialog(app: JobApplication, event: Event): void {
    event.stopPropagation();
    const ref = this.dialog.open(ApplicationForm, { data: { application: app }, width: '520px' });
    ref.afterClosed().subscribe((updated) => {
      if (updated) this.load();
    });
  }

  deleteApplication(app: JobApplication, event: Event): void {
    event.stopPropagation();
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete application',
        message: `Delete the application to ${app.companyName} for ${app.jobTitle}? This cannot be undone.`,
        confirmLabel: 'Delete'
      }
    });

    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.applicationService.deleteApplication(app.id).subscribe(() => this.load());
      }
    });
  }

  viewApplication(app: JobApplication): void {
    this.router.navigate([ApplicationRoute.Detail(app.id)]);
  }
}
