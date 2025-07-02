import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SearchRequest } from '../models/search-request.model';
import { StoryModel } from '../models/story.model';

@Injectable({
  providedIn: 'root',
})
export class HackerNewsService {
  private apiUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) {}

  public getFilteredNews(
    searchRequest: SearchRequest,
  ): Observable<StoryModel[]> {
    return this.httpClient.post<StoryModel[]>(
      `${this.apiUrl}/stories/newest-stories`,
      searchRequest,
    );
  }
}
