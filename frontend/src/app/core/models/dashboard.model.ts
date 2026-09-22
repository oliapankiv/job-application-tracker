export interface StatusCount {
  status: string;
  count: number;
}

export interface WeeklyCount {
  weekStart: string;
  count: number;
}

export interface DashboardStats {
  totalApplications: number;
  applicationsThisMonth: number;
  applicationsThisWeek: number;
  responseRate: number;
  activeInterviews: number;
  offersReceived: number;
  statusBreakdown: StatusCount[];
  applicationsPerWeek: WeeklyCount[];
}
