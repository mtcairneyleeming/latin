import * as models from './models';

export interface LemmaData {
  'lemmaId'?: number;
  'partOfSpeechId'?: number;
  'categoryId'?: number;
  'genderId'?: number;
  'useSingular'?: boolean;
  'category'?: models.Category;
  'gender'?: models.Gender;
  'lemma'?: models.Lemma;
  'partOfSpeech'?: models.PartOfSpeech;
}

