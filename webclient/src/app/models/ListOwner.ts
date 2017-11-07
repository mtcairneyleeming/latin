import * as models from './models';

export interface ListOwner {
  'userId'?: string;
  'listId'?: number;
  'isOwner'?: boolean;
  'list'?: models.List;
}

