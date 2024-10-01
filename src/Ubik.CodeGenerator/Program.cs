// See https://aka.ms/new-console-template for more information
using Ubik.CodeGenerator;
using Ubik.Security.Api.Data;

var generator = new ClassGenerator(typeof(SecurityDbContext));
generator.GenerateClassesContractAddCommand();
