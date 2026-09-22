export interface Reminder {
  id: number;
  applicationId: number;
  companyName: string;
  jobTitle: string;
  dueDate: string;
  message: string;
  isCompleted: boolean;
}

export interface CreateReminder {
  dueDate: string;
  message: string;
}

export interface UpdateReminder extends CreateReminder {
  isCompleted: boolean;
}
