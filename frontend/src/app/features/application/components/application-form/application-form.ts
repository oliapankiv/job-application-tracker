import { Component, Inject, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApplicationService } from '../../services/application.service';
import {
  ApplicationSource,
  JobApplication,
  SOURCE_LABELS,
  WORK_TYPE_LABELS,
  WorkType
} from '../../../../shared/features/application/models/application.model';

export interface ApplicationFormData {
  application?: JobApplication | null;
}

@Component({
  selector: 'app-application-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './application-form.html',
  styleUrl: './application-form.scss'
})
export class ApplicationForm {
  private fb = inject(FormBuilder);

  readonly saving = signal(false);
  readonly isEdit: boolean;

  readonly sourceOptions = Object.entries(SOURCE_LABELS).map(([value, label]) => ({
    value: Number(value) as ApplicationSource,
    label
  }));

  readonly workTypeOptions = Object.entries(WORK_TYPE_LABELS).map(([value, label]) => ({
    value: Number(value) as WorkType,
    label
  }));

  readonly form = this.fb.group({
    companyName: ['', Validators.required],
    jobTitle: ['', Validators.required],
    jobPostingUrl: [''],
    source: [ApplicationSource.LinkedIn, Validators.required],
    workType: [WorkType.Remote, Validators.required],
    salary: [null as number | null],
    location: [''],
    appliedDate: [new Date(), Validators.required],
    notes: [''],
    tags: ['']
  });

  constructor(
    private applicationService: ApplicationService,
    private dialogRef: MatDialogRef<ApplicationForm, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: ApplicationFormData
  ) {
    this.isEdit = !!data.application;

    if (data.application) {
      const app = data.application;
      this.form.patchValue({
        companyName: app.companyName,
        jobTitle: app.jobTitle,
        jobPostingUrl: app.jobPostingUrl ?? '',
        source: app.source,
        workType: app.workType,
        salary: app.salary ?? null,
        location: app.location ?? '',
        appliedDate: new Date(app.appliedDate),
        notes: app.notes ?? '',
        tags: app.tags ?? ''
      });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const appliedDate = (raw.appliedDate as Date).toISOString();

    this.saving.set(true);

    const payload = {
      companyName: raw.companyName!,
      jobTitle: raw.jobTitle!,
      jobPostingUrl: raw.jobPostingUrl || null,
      source: raw.source!,
      workType: raw.workType!,
      salary: raw.salary,
      location: raw.location || null,
      notes: raw.notes || null,
      tags: raw.tags || null
    };

    const request: Observable<unknown> = this.isEdit
      ? this.applicationService.updateApplication(this.data.application!.id, { ...payload, appliedDate })
      : this.applicationService.createApplication({ ...payload, appliedDate });

    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.dialogRef.close(true);
      },
      error: () => this.saving.set(false)
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
