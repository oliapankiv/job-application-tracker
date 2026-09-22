namespace JobTracker.Api.Models;

public enum ApplicationStatus
{
    Applied = 0,
    Screening = 1,
    Interview = 2,
    Offer = 3,
    Rejected = 4
}

public enum ApplicationSource
{
    LinkedIn = 0,
    CompanyWebsite = 1,
    Referral = 2,
    JobBoard = 3,
    Recruiter = 4,
    Other = 5
}

public enum WorkType
{
    Remote = 0,
    Hybrid = 1,
    Onsite = 2
}

public enum DocumentType
{
    Resume = 0,
    CoverLetter = 1,
    Other = 2
}
