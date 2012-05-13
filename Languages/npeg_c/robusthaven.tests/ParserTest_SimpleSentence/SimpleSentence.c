#include <string.h>
#include <stdlib.h>
#include "SimpleSentence.h"

static int _subject_verb_n_object(npeg_inputiterator* iterator, npeg_context* context);
static int _subject(npeg_inputiterator* iterator, npeg_context* context);
static int _match_subjects(npeg_inputiterator* iterator, npeg_context* context);
static int _match_I(npeg_inputiterator* iterator, npeg_context* context);
static int _match_You(npeg_inputiterator* iterator, npeg_context* context);
static int _verb_n_object(npeg_inputiterator* iterator, npeg_context* context);
static int _object(npeg_inputiterator* iterator, npeg_context* context);
static int _article(npeg_inputiterator* iterator, npeg_context* context);
static int _match_the(npeg_inputiterator* iterator, npeg_context* context);
static int _match_undefined_article(npeg_inputiterator* iterator, npeg_context* context);
static int _match_an(npeg_inputiterator* iterator, npeg_context* context);
static int _noun(npeg_inputiterator* iterator, npeg_context* context);
static int _match_drivable(npeg_inputiterator* iterator, npeg_context* context);
static int _match_car(npeg_inputiterator* iterator, npeg_context* context);
static int _match_bus(npeg_inputiterator* iterator, npeg_context* context);
static int _match_edible(npeg_inputiterator* iterator, npeg_context* context);
static int _match_apple(npeg_inputiterator* iterator, npeg_context* context);
static int _match_a(npeg_inputiterator* iterator, npeg_context* context);
static int _match_steak(npeg_inputiterator* iterator, npeg_context* context);
static int _match_verbs(npeg_inputiterator* iterator, npeg_context* context);
static int _match_go_or_eat(npeg_inputiterator* iterator, npeg_context* context);
static int _match_go(npeg_inputiterator* iterator, npeg_context* context);
static int _match_eat(npeg_inputiterator* iterator, npeg_context* context);
static int _match_drive_or_walk(npeg_inputiterator* iterator, npeg_context* context);
static int _match_drive(npeg_inputiterator* iterator, npeg_context* context);
static int _verb(npeg_inputiterator* iterator, npeg_context* context);
static int _match_walk(npeg_inputiterator* iterator, npeg_context* context);

int sentence_root(npeg_inputiterator* iterator, npeg_context* context) {
  char* _nodeName_0 = "Sentence";

  return npeg_CapturingGroup(iterator,  context, &_subject_verb_n_object, _nodeName_0, 0, 0);
}

static int _subject_verb_n_object(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_Sequence(iterator,  context, &_subject, &_verb_n_object);
}

static int _subject(npeg_inputiterator* iterator, npeg_context* context) {
  char* _nodeName_1 = "subject";

  return npeg_CapturingGroup(iterator,  context, &_match_subjects, _nodeName_1, 0, 0);
}

static int _match_subjects(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator,  context, &_match_I, &_match_You);
}

static int _match_I(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "I ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_You(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "You ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _verb_n_object(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_Sequence(iterator, context, _verb, _object);
} 

static int _match_objects(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_Sequence(iterator, context, _article, _noun);
}

static int _object(npeg_inputiterator* iterator, npeg_context* context) {
  char* _nodeName_1 = "object";

  return npeg_CapturingGroup(iterator,  context, &_match_objects, _nodeName_1, 0, 0);
} 

static int _article(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator, context, _match_undefined_article, _match_the);
}

static int _match_the(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "the ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_undefined_article(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator, context, _match_an, _match_a);
}

static int _match_an(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "an ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _noun(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator, context, _match_drivable, _match_edible);
}

static int _match_drivable(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator, context, _match_car, _match_bus);
} 

static int _match_car(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "car.";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_bus(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "bus.";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_edible(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator, context, _match_apple, _match_steak);
} 

static int _match_apple(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "apple.";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_steak(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "steak.";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_a(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "a ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_verbs(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator,  context, &_match_go_or_eat, &_match_drive_or_walk);
}

static int _match_go_or_eat(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator,  context, &_match_go, &_match_eat);
}

static int _match_go(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "go ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_eat(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "eat ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_drive_or_walk(npeg_inputiterator* iterator, npeg_context* context) {
  return npeg_PrioritizedChoice(iterator,  context, &_match_drive, &_match_walk);
}

static int _match_drive(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "drive ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _match_walk(npeg_inputiterator* iterator, npeg_context* context) {
  char text[] = "walk ";

  return npeg_Literal(iterator, context, text, strlen(text), 0);
}

static int _verb(npeg_inputiterator* iterator, npeg_context* context) {
  char* _nodeName_2 = "verb";

  return npeg_CapturingGroup(iterator,  context, &_match_verbs, _nodeName_2, 0, 0);
} 
