/**
 * magister api
 * api for magister
 *
 * OpenAPI spec version: v1
 *
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */

import * as models from './models';

export interface Lemmas {
  'lemmaId'?: number;
  'lemmaText'?: string;
  'lemmaShortDef'?: string;
  'lemmaData'?: models.LemmaData;
  'definition'?: Array<models.Definition>;
  'forms'?: Array<models.Forms>;
  'sectionWords'?: Array<models.SectionWords>;
  'userLearntWords'?: Array<models.UserLearntWords>;
}
