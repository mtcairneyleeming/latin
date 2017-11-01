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

export interface UserLearntWords {
  'userId'?: string;
  'lemmaId'?: number;
  'learntPercentage'?: number;
  'nextRevision'?: Date;
  'revisionStage'?: number;
  'lemma'?: models.Lemmas;
}

