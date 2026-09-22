import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Contact, CreateContact } from '../../../shared/features/application/models/application.model';
import { ContactConfig } from '../configs/contact.config';

@Injectable({ providedIn: 'root' })
export class ContactService {
  constructor(private http: HttpClient) {}

  getContacts(applicationId: number): Observable<Contact[]> {
    return this.http.get<Contact[]>(ContactConfig.LIST(applicationId));
  }

  createContact(applicationId: number, dto: CreateContact): Observable<Contact> {
    return this.http.post<Contact>(ContactConfig.LIST(applicationId), dto);
  }

  updateContact(contactId: number, dto: CreateContact): Observable<void> {
    return this.http.put<void>(ContactConfig.ITEM(contactId), dto);
  }

  deleteContact(contactId: number): Observable<void> {
    return this.http.delete<void>(ContactConfig.ITEM(contactId));
  }
}
