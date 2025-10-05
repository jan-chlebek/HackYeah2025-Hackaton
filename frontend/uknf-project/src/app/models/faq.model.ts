/**
 * FAQ Question Status enumeration
 */
export enum FaqQuestionStatus {
  Submitted = 0,
  InProgress = 1,
  Answered = 2,
  Published = 3,
  Rejected = 4
}

/**
 * FAQ Question model
 */
export interface FaqQuestion {
  id: number;
  title: string;
  content: string;
  category: string;
  tags: string;
  status: FaqQuestionStatus;
  answerContent: string | null;
  answeredAt: string | null;
  answeredByUserId: number | null;
  submittedByUserId: number | null;
  anonymousName: string | null;
  anonymousEmail: string | null;
  submittedAt: string;
  publishedAt: string | null;
  viewCount: number;
  averageRating: number | null;
  ratingCount: number;
}

/**
 * FAQ Rating model
 */
export interface FaqRating {
  id: number;
  faqQuestionId: number;
  userId: number;
  rating: number;
  ratedAt: string;
}

/**
 * FAQ list response with pagination
 */
export interface FaqListResponse {
  items: FaqQuestion[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
