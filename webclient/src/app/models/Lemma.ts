import * as models from './models';

export interface Lemma {
  'lemmaId'?: number;
  'lemmaText'?: string;
  'lemmaShortDef'?: string;
  'lemmaData'?: models.LemmaData;
  'definition'?: Array<models.Definition>;
  'forms'?: Array<models.Form>;
  'sectionWords'?: Array<models.SectionWord>;
  'userLearntWords'?: Array<models.UserLearntWord>;
}

