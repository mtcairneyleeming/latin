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

export interface Forms {
  'id'?: number;
  'lemmaId'?: number;
  'morphCode'?: string;
  'form'?: string;
  'miscFeatures'?: string;
  'lemma'?: models.Lemmas;
}

