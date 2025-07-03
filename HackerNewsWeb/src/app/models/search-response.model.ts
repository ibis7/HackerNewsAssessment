import { StoryModel } from './story.model';

export interface SearchResponse {
  stories: StoryModel[];
  totalLength: number;
}
