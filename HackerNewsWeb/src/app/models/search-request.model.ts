export interface SearchRequest {
  searchTerm?: string;
  sortedBy?: string;
  isSortingAscending?: boolean;
  pageSize: number;
  pageNumber: number;
}
