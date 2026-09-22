import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CreateContact } from '../../../../shared/features/application/models/application.model';

@Component({
  selector: 'app-contact-form-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './contact-form-dialog.html'
})
export class ContactFormDialog {
  private fb = inject(FormBuilder);

  readonly form = this.fb.group({
    name: ['', Validators.required],
    role: [''],
    email: ['', Validators.email],
    phone: [''],
    linkedInUrl: ['']
  });

  constructor(private dialogRef: MatDialogRef<ContactFormDialog, CreateContact>) {}

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.dialogRef.close({
      name: raw.name!,
      role: raw.role || null,
      email: raw.email || null,
      phone: raw.phone || null,
      linkedInUrl: raw.linkedInUrl || null
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
