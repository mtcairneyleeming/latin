import * as models from './models';

export interface Result {
  // Note: a somewhat messy and less maintainable line - maybe a better solution?
  'data'?: Array<models.Lemma> | Array<models.List> | Array<models.Section> | Object;
  'success'?: boolean;
  'errorMessage'?: string;
  'exception'?: any;
}

