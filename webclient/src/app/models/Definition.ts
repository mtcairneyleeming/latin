import * as models from './models';

export interface Definition {
  'definitionId'?: number;
  'lemmaId'?: number;
  'alevel'?: string;
  'lemma'?: models.Lemma;
}

