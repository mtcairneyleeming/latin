import * as models from './models';

export interface Form {
  'id'?: number;
  'lemmaId'?: number;
  'morphCode'?: string;
  'form'?: string;
  'miscFeatures'?: string;
  'lemma'?: models.Lemma;
}

