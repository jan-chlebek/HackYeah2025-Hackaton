#!/bin/bash

###############################################################################
# UKNF Communication Platform - Run Quick Tests
###############################################################################
# Runs only unit tests (no integration tests) for faster feedback
###############################################################################

./run-tests.sh --unit-only "$@"
