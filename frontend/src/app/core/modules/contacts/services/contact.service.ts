import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { Contact, CreateContact } from '../../../models/application.model';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getContacts(applicationId: number): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${this.baseUrl}/applications/${applicationId}/contacts`);
  }

  createContact(applicationId: number, dto: CreateContact): Observable<Contact> {
    return this.http.post<Contact>(`${this.baseUrl}/applications/${applicationId}/contacts`, dto);
  }

  updateContact(contactId: number, dto: CreateContact): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/contacts/${contactId}`, dto);
  }

  deleteContact(contactId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/contacts/${contactId}`);
  }
}
