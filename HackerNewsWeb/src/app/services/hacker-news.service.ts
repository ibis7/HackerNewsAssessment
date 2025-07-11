import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SearchRequest } from '../models/search-request.model';
import { SearchResponse } from '../models/search-response.model';

@Injectable({
  providedIn: 'root',
})
export class HackerNewsService {
  private apiUrl = environment.apiUrl;

  private httpClient = inject(HttpClient);

  public getFilteredNews(
    searchRequest: SearchRequest,
  ): Observable<SearchResponse> {
    return this.httpClient.post<SearchResponse>(
      `${this.apiUrl}/stories/newest-stories`,
      searchRequest,
    );
  }
}
