/**
 * FAQ Question model - matches API response format
 */
export interface FaqQuestion {
  id: number;
  question: string;
  answer: string;
}

/**
 * FAQ list response with pagination
 */
export interface FaqListResponse {
  items: FaqQuestion[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
