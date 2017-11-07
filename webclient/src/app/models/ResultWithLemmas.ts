import * as models from './models';

export interface ResultWithLemmas {
  'data'?: Array<models.Lemma>;
  'success'?: boolean;
  'errorMessage'?: string;
  'exception'?: any;
}

