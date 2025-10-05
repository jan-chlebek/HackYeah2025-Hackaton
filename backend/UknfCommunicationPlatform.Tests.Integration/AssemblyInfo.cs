using Xunit;

// Disable parallelization to avoid database state races during integration tests.
// Heavy TRUNCATE/reset operations plus shared singleton fixtures were causing timing issues
// (e.g., partial truncation while another test generated JWT claims). Serializing tests ensures
// deterministic DB state. If performance becomes a concern later, migrate to per-test schemas
// or transactional rollbacks instead of global truncation.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
