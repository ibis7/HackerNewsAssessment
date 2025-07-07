import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { of, throwError } from 'rxjs';
import { HackerNewsService } from '../../services/hacker-news.service';
import { HackerNewsStoriesComponent } from './hacker-news-stories.component';

describe('HackerNewsStoriesComponent', () => {
  let component: HackerNewsStoriesComponent;
  let fixture: ComponentFixture<HackerNewsStoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HackerNewsStoriesComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        HackerNewsService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HackerNewsStoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should call resetRequest and searchData on ngOnInit', () => {
      const resetRequestSpy = spyOn(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component as any,
        'resetRequest',
      ).and.callThrough();

      const searchDataSpy = spyOn(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component as any,
        'searchData',
      ).and.callThrough();

      component.ngOnInit();

      expect(resetRequestSpy).toHaveBeenCalled();
      expect(searchDataSpy).toHaveBeenCalled();
    });

    it('should set request to default values after ngOnInit', () => {
      component.request = { pageSize: 999, pageNumber: 999 };
      component.ngOnInit();
      expect(component.request.pageSize).toBe(component.pageSizeOptions[0]);
      expect(component.request.pageNumber).toBe(0);
    });
  });

  describe('ngAfterViewInit', () => {
    it('should call subscribeToSearchInput', () => {
      const subscribeToSearchInputSpy = spyOn(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component as any,
        'subscribeToSearchInput',
      ).and.callThrough();

      component.ngAfterViewInit();

      expect(subscribeToSearchInputSpy).toHaveBeenCalled();
    });

    it('should subscribe to searchControl valueChanges and call searchData on input', (done) => {
      const searchDataSpy = spyOn(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        component as any,
        'searchData',
      ).and.callThrough();

      component.ngAfterViewInit();

      component.searchControl.setValue('test');

      setTimeout(() => {
        expect(searchDataSpy).toHaveBeenCalled();
        expect(component.request.searchTerm).toBe('test');
        done();
      }, 1100);
    });
  });

  describe('sortChange', () => {
    let searchDataSpy: jasmine.Spy;

    beforeEach(() => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      searchDataSpy = spyOn(component as any, 'searchData').and.callThrough();
    });

    it('should set isSortingAscending to true and sortedBy to event.active when direction is "asc"', () => {
      const sortEvent = { active: 'title', direction: 'asc' } as Sort;
      component.sortChange(sortEvent);

      expect(component.request.isSortingAscending).toBeTrue();
      expect(component.request.sortedBy).toBe('title');
      expect(searchDataSpy).toHaveBeenCalled();
    });

    it('should set isSortingAscending to false and sortedBy to event.active when direction is "desc"', () => {
      const sortEvent = { active: 'url', direction: 'desc' } as Sort;
      component.sortChange(sortEvent);

      expect(component.request.isSortingAscending).toBeFalse();
      expect(component.request.sortedBy).toBe('url');
      expect(searchDataSpy).toHaveBeenCalled();
    });

    it('should set isSortingAscending and sortedBy to undefined when direction is empty', () => {
      const sortEvent = { active: 'title', direction: '' } as Sort;
      component.sortChange(sortEvent);

      expect(component.request.isSortingAscending).toBeUndefined();
      expect(component.request.sortedBy).toBeUndefined();
      expect(searchDataSpy).toHaveBeenCalled();
    });
  });

  describe('changePaging', () => {
    let searchDataSpy: jasmine.Spy;

    beforeEach(() => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      searchDataSpy = spyOn(component as any, 'searchData').and.callThrough();
    });

    it('should update request.pageNumber and request.pageSize and call searchData', () => {
      const pageEvent = { pageIndex: 2, pageSize: 50 } as PageEvent;
      component.changePaging(pageEvent);

      expect(component.request.pageNumber).toBe(2);
      expect(component.request.pageSize).toBe(50);
      expect(searchDataSpy).toHaveBeenCalled();
    });

    it('should handle zero pageIndex and minimum pageSize', () => {
      const pageEvent = { pageIndex: 0, pageSize: 10 } as PageEvent;
      component.changePaging(pageEvent);

      expect(component.request.pageNumber).toBe(0);
      expect(component.request.pageSize).toBe(10);
      expect(searchDataSpy).toHaveBeenCalled();
    });
  });

  describe('searchData', () => {
    let hackerNewsService: HackerNewsService;
    const mockResponse = {
      stories: [{ title: 'story', url: 'url', id: 1 }],
      totalLength: 1,
    };

    beforeEach(() => {
      hackerNewsService = TestBed.inject(HackerNewsService);
    });

    it('should set isLoading true while loading and update response on success', () => {
      spyOn(hackerNewsService, 'getFilteredNews').and.returnValue(
        of(mockResponse),
      );
      component.isLoading = false;
      component.response = { stories: [], totalLength: 0 };

      component.sortChange({ active: 'title', direction: 'asc' });

      expect(component.isLoading).toBeFalse();
      expect(component.response).toEqual(mockResponse);
    });

    it('should set response to default and isLoading false on error', () => {
      spyOn(hackerNewsService, 'getFilteredNews').and.returnValue(
        throwError(() => new Error('fail')),
      );
      component.response = {
        stories: [{ title: 'old', url: 'old', id: 2 }],
        totalLength: 1,
      };
      component.isLoading = false;

      component.changePaging({ pageIndex: 0, pageSize: 10 } as PageEvent);

      expect(component.isLoading).toBeFalse();
      expect(component.response).toEqual({ stories: [], totalLength: 0 });
    });

    it('should call table.renderRows if table is defined', () => {
      spyOn(hackerNewsService, 'getFilteredNews').and.returnValue(
        of(mockResponse),
      );

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      component.table = { renderRows: jasmine.createSpy('renderRows') } as any;

      component.sortChange({ active: 'title', direction: 'asc' });

      expect(component.table?.renderRows).toHaveBeenCalled();
    });
  });
});
