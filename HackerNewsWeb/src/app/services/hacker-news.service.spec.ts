import { TestBed } from '@angular/core/testing';

import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { SearchRequest } from '../models/search-request.model';
import { SearchResponse } from '../models/search-response.model';
import { HackerNewsService } from './hacker-news.service';

describe('HackerNewsService', () => {
  let service: HackerNewsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        HackerNewsService,
      ],
    });

    service = TestBed.inject(HackerNewsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call the correct API endpoint with the provided searchRequest', () => {
    const mockRequest: SearchRequest = {
      searchTerm: 'test',
      pageNumber: 1,
      pageSize: 20,
    };

    const mockResponse: SearchResponse = {
      stories: [
        {
          title: 'test',
          url: 'https://test.com',
          id: 1,
        },
      ],
      totalLength: 20,
    };

    let actualResponse: SearchResponse | undefined;

    service.getFilteredNews(mockRequest).subscribe((response) => {
      actualResponse = response;
    });

    const req = httpMock.expectOne(
      `${environment.apiUrl}/stories/newest-stories`,
    );
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockRequest);

    req.flush(mockResponse);

    expect(actualResponse).toEqual(mockResponse);

    httpMock.verify();
  });
});
