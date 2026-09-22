import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Contact, CreateContact } from '../../../shared/features/application/models/application.model';

@Injectable({ providedIn: 'root' })
export class ContactService {
  constructor(private http: HttpClient) {}

  getContacts(applicationId: number): Observable<Contact[]> {
    return this.http.get<Contact[]>(`/applications/${applicationId}/contacts`);
  }

  createContact(applicationId: number, dto: CreateContact): Observable<Contact> {
    return this.http.post<Contact>(`/applications/${applicationId}/contacts`, dto);
  }

  updateContact(contactId: number, dto: CreateContact): Observable<void> {
    return this.http.put<void>(`/contacts/${contactId}`, dto);
  }

  deleteContact(contactId: number): Observable<void> {
    return this.http.delete<void>(`/contacts/${contactId}`);
  }
}
