import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { CreateReminder } from '../../../../shared/features/reminder/models/reminder.model';

@Component({
  selector: 'app-reminder-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './reminder-form-dialog.html'
})
export class ReminderFormDialog {
  private fb = inject(FormBuilder);
  private readonly inSevenDays = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);

  readonly form = this.fb.group({
    dueDate: [this.inSevenDays, Validators.required],
    message: ['Follow up on application', Validators.required]
  });

  constructor(private dialogRef: MatDialogRef<ReminderFormDialog, CreateReminder>) {}

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.dialogRef.close({
      dueDate: (raw.dueDate as Date).toISOString(),
      message: raw.message!
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
