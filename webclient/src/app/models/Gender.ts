import * as models from './models';

export interface Gender {
  'genderId'?: number;
  'genderCode'?: string;
  'name'?: string;
  'description'?: string;
  'lemmaData'?: Array<models.LemmaData>;
}

