import * as models from './models';

export interface Category {
  'categoryId'?: number;
  'name'?: string;
  'number'?: number;
  'categoryIdentifier'?: string;
  'lemmaData'?: Array<models.LemmaData>;
}

