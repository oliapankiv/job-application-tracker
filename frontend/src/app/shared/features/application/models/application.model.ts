import { Reminder } from '../../reminder/models/reminder.model';

export enum ApplicationStatus {
  Applied = 0,
  Screening = 1,
  Interview = 2,
  Offer = 3,
  Rejected = 4
}

export enum ApplicationSource {
  LinkedIn = 0,
  CompanyWebsite = 1,
  Referral = 2,
  JobBoard = 3,
  Recruiter = 4,
  Other = 5
}

export enum WorkType {
  Remote = 0,
  Hybrid = 1,
  Onsite = 2
}

export const APPLICATION_STATUSES: ApplicationStatus[] = [
  ApplicationStatus.Applied,
  ApplicationStatus.Screening,
  ApplicationStatus.Interview,
  ApplicationStatus.Offer,
  ApplicationStatus.Rejected
];

export const STATUS_LABELS: Record<ApplicationStatus, string> = {
  [ApplicationStatus.Applied]: 'Applied',
  [ApplicationStatus.Screening]: 'Screening',
  [ApplicationStatus.Interview]: 'Interview',
  [ApplicationStatus.Offer]: 'Offer',
  [ApplicationStatus.Rejected]: 'Rejected'
};

export const SOURCE_LABELS: Record<ApplicationSource, string> = {
  [ApplicationSource.LinkedIn]: 'LinkedIn',
  [ApplicationSource.CompanyWebsite]: 'Company Website',
  [ApplicationSource.Referral]: 'Referral',
  [ApplicationSource.JobBoard]: 'Job Board',
  [ApplicationSource.Recruiter]: 'Recruiter',
  [ApplicationSource.Other]: 'Other'
};

export const WORK_TYPE_LABELS: Record<WorkType, string> = {
  [WorkType.Remote]: 'Remote',
  [WorkType.Hybrid]: 'Hybrid',
  [WorkType.Onsite]: 'Onsite'
};

export interface StatusHistoryEntry {
  id: number;
  status: ApplicationStatus;
  changedAt: string;
  note?: string | null;
}

export interface JobApplication {
  id: number;
  companyName: string;
  jobTitle: string;
  jobPostingUrl?: string | null;
  status: ApplicationStatus;
  source: ApplicationSource;
  salary?: number | null;
  location?: string | null;
  workType: WorkType;
  appliedDate: string;
  lastUpdated: string;
  notes?: string | null;
  tags?: string | null;
}

export interface CreateJobApplication {
  companyName: string;
  jobTitle: string;
  jobPostingUrl?: string | null;
  source: ApplicationSource;
  salary?: number | null;
  location?: string | null;
  workType: WorkType;
  appliedDate?: string | null;
  notes?: string | null;
  tags?: string | null;
}

export interface UpdateJobApplication extends CreateJobApplication {
  appliedDate: string;
}

export interface UpdateStatusRequest {
  status: ApplicationStatus;
  note?: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Contact {
  id: number;
  applicationId: number;
  name: string;
  role?: string | null;
  email?: string | null;
  phone?: string | null;
  linkedInUrl?: string | null;
}

export interface CreateContact {
  name: string;
  role?: string | null;
  email?: string | null;
  phone?: string | null;
  linkedInUrl?: string | null;
}


export enum DocumentType {
  Resume = 0,
  CoverLetter = 1,
  Other = 2
}

export interface JobDocument {
  id: number;
  applicationId: number;
  fileName: string;
  filePath: string;
  documentType: DocumentType;
  uploadedAt: string;
}

export interface JobApplicationDetail extends JobApplication {
  statusHistory: StatusHistoryEntry[];
  contacts: Contact[];
  reminders: Reminder[];
  documents: JobDocument[];
}
