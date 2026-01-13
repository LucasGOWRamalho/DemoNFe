﻿using DFe.Utils;
using HerculesZeusDfeDemo;
using NFe.Classes;
using NFe.Classes.Protocolo;
using NFe.Classes.Servicos.Tipos;
using NFe.Danfe.Base.NFe;
using NFe.Danfe.OpenFast.NFe;
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.NFe;
using NFe.Utils.Validacao;
using System.Net;

Console.WriteLine("=== HERCULES ZEUS - EMISSOR DE DANFE DEMO ===");
Console.WriteLine("GERANDO NFe DE TESTE...");

// 1. GERAR NFe
var nfe = FactoryNfe.Gerar(serie: 32, nro: 121);
Console.WriteLine($"✅ NFe Gerada - Número: {nfe.infNFe.ide.nNF}, Série: {nfe.infNFe.ide.serie}");
Console.WriteLine($"   Emitente: {nfe.infNFe.emit.xNome}");
Console.WriteLine($"   Destinatário: {nfe.infNFe.dest.xNome}");
Console.WriteLine($"   Valor Total: R$ {nfe.infNFe.total.ICMSTot.vNF:F2}");

// 2. MOSTRAR XML (SEM ASSINATURA)
var xmlNfe = nfe.ObterXmlString();
Console.WriteLine($"\n📄 XML gerado ({xmlNfe.Length} caracteres)");
Helpers.AbrirXml(xmlNfe);
Console.WriteLine("\n🔍 XML aberto para visualização. Pressione qualquer tecla para continuar...");
Console.ReadKey();

// 3. CRIAR nfeProc PARA O DANFE
Console.WriteLine("\n🔄 Criando objeto nfeProc para DANFE...");
var nfeprocXml = new nfeProc
{
    NFe = nfe,
    versao = "4.00",
    protNFe = new protNFe
    {
        infProt = new infProt
        {
            tpAmb = DFe.Classes.Flags.TipoAmbiente.Homologacao,
            verAplic = "1.00",
            chNFe = $"NFe{new Random().Next(100000000, 999999999)}",
            dhRecbto = DateTime.Now,
            nProt = "123456789012345",
            digVal = "TESTE",
            cStat = 100,
            xMotivo = "Autorizado o uso da NF-e (MODO DEMO)"
        },
        versao = "4.00"
    }
};

// 4. CONFIGURAR E GERAR DANFE
Console.WriteLine("🎨 Configurando DANFE...");
var configDanfe = new ConfiguracaoDanfeNfe()
{
    Logomarca = null,
    DuasLinhas = false,
    DocumentoCancelado = false,
    QuebrarLinhasObservacao = true,
    ExibirResumoCanhoto = true,
    ResumoCanhoto = "DOCUMENTO EMITIDO EM MODO DE DEMONSTRAÇÃO - SEM VALOR FISCAL",
    ChaveContingencia = "",
    ExibeCampoFatura = false,
    ImprimirISSQN = true,
    ImprimirDescPorc = true,
    ImprimirTotalLiquido = true,
    ImprimirUnidQtdeValor = ImprimirUnidQtdeValor.Comercial,
    ExibirTotalTributos = true,
    ExibeRetencoes = false
};

Console.WriteLine("🖨️  Gerando PDF do DANFE...");
try
{
    var danfe = new DanfeFrNfe(
        proc: nfeprocXml,
        configuracaoDanfeNfe: configDanfe,
        desenvolvedor: "HERCULES ZEUS DEMO",
        arquivoRelatorio: string.Empty);
    
    byte[] pdfBytes = danfe.ExportarPdf();
    Console.WriteLine($"✅ PDF gerado com sucesso! Tamanho: {pdfBytes.Length} bytes");

    // 5. SALVAR PDF
    string desktopPath = @"C:\Users\lucasgow\Desktop\Teste Hercules";
    Directory.CreateDirectory(desktopPath);
    
    string nomeArquivo = $"DANFE_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
    string caminhoCompleto = Path.Combine(desktopPath, nomeArquivo);
    
    File.WriteAllBytes(caminhoCompleto, pdfBytes);
    Console.WriteLine($"💾 PDF salvo em: {caminhoCompleto}");

    // 6. ABRIR PDF
    Console.WriteLine("🔓 Abrindo PDF gerado...");
    Helpers.AbrirPdf(pdfBytes);

    // 7. RESUMO FINAL
    Console.WriteLine("\n" + new string('=', 50));
    Console.WriteLine("🎉 PROCESSO CONCLUÍDO COM SUCESSO!");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine("\n📋 RESUMO DA OPERAÇÃO:");
    Console.WriteLine($"   1. ✅ NFe gerada: Série {nfe.infNFe.ide.serie}, Nº {nfe.infNFe.ide.nNF}");
    Console.WriteLine($"   2. ✅ XML visualizado: {xmlNfe.Length} caracteres");
    Console.WriteLine($"   3. ✅ DANFE criado: {nomeArquivo}");
    Console.WriteLine($"   4. ✅ PDF salvo: {caminhoCompleto}");
    Console.WriteLine($"   5. ✅ Valor total: R$ {nfe.infNFe.total.ICMSTot.vNF:F2}");
    Console.WriteLine($"\n📌 Local do arquivo: {desktopPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ ERRO AO GERAR DANFE: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Detalhes: {ex.InnerException.Message}");
        Console.WriteLine($"   Stack trace: {ex.InnerException.StackTrace}");
    }
    Console.WriteLine("\n🏁 Pressione qualquer tecla para encerrar...");
    Console.ReadKey();
    return 1;
}

Console.WriteLine("\n🏁 Pressione qualquer tecla para encerrar...");
Console.ReadKey();
return 0;