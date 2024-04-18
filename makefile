
format:
	@fd cs$$ -X dotnet csharpier {}
	@fd cs$$ -X dos2unix -q -r {}
	@fd csproj$$ -X dos2unix -q -r {}
