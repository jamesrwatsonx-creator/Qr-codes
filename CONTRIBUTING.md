# Contributing

Contributions are more than welcome. Flink is meant to be a project for the community.
To keep the entry-barrier for contributors as small as possible, there are only 5 rules to follow.

1. Create an Issue in the [Flink Issue tracker](https://gitlab.com/rtraceio/web/flink/-/issues/new) (if it doesn't exist already)
2. Describe the problem you want to have solved
3. [Fork](https://gitlab.com/rtraceio/web/flink/-/forks/new) Flink and start implementation
4. Create a Pull Request, briefly (2-5 sentences) describe your changes, and assing it to @rehberger.raffael
5. Make sure all Unit Tests, and Integration Tests are passing

That's it - not more, not less.


## Some Recommendations

- prevent breaking changes of the HTTP API, unless there's no way around it
- keep your code clean - Spaghetti Code might get the job done, but makes other devs depressive
- keep your code testable - in the best case even write unit/integration tests for your changes
- unless absolutely necessary, prevent addition of further dependencies (e.g. NuGet packages, JS libraries)


## Contact

If you have any questions, please reach out to: [flink@rtrace.io](mailto:flink@rtrace.io)
