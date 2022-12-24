=== Using this program
1. download transaction files from banks and save them into "transactions/bank" folder, you can create subfolders
2. run this program to get transactions/sorted-*.csv files
3. add categories to sorted-*.csv files and add Description
	3.a. if you want to split a transaction, put Category=Exclude, and manually add more transactions, change id
4. run this program again to get transactions/output-*.csv files


=== folder structure
D:transactions
  D:bank
  D:user
	F:Categories.json
	F:sorted*.csv
  F:output*.csv
    
=== Format of sorted.csv
Id, Date, Description, Amount, Category

=== Format of Categories.json
{
   "Category" : [
		"substring in a transaction to match",
		"/regex to match in a transaction"
   ]
}


